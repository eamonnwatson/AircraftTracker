using AircraftTracker.Interfaces;
using AircraftTracker.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net;

namespace AircraftTracker;
internal class FlightAware : IFlightParser
{
    private const string FA_URL = "https://flightaware.com/live/airport/{0}";
    //private const string NODE_SELECTOR = "//div[@class='airportBoardContainer']";
    private const string XPATH = "/html[1]/div[1]/div[1]/div[2]/div[1]/div[3]/div[1]/div[3]/div[1]/div[1]";

    private readonly ILogger<FlightAware> logger;

    public FlightAware(ILogger<FlightAware> logger)
    {
        this.logger = logger;
    }
    public async Task<IEnumerable<LiveFlight>> GetFlightsAsync(string airportICAO, CancellationToken cancellationToken)
    {
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(string.Format(FA_URL, airportICAO), cancellationToken);
        try
        {
            //var boards = doc.DocumentNode.SelectNodes(NODE_SELECTOR);
            //var enroute = boards.FirstOrDefault(a => a.InnerHtml.Contains("En Route/Scheduled", StringComparison.InvariantCultureIgnoreCase));

            var enroute = doc.DocumentNode.SelectSingleNode(XPATH);

            if (enroute is null)
                return Enumerable.Empty<LiveFlight>();

            var items = GetFlightsFromTable(enroute);

            logger.LogInformation("Retrieved {FlightCount} flights", items.Count());

            return items;
        }
        catch (Exception ex)
        {
            throw new FlightException("A Parsing Error occured parsing flightaware.com", ex);
        }
    }

    private IEnumerable<LiveFlight> GetFlightsFromTable(HtmlNode enroute)
    {
        var rows = enroute.FirstChild.Elements("tr");

        if (rows is null || !rows.Any())
            return Enumerable.Empty<LiveFlight>();

        return rows.Select(r => ConvertRowToFlight(r)).Where(r => r is not null)!;
    }

    private LiveFlight? ConvertRowToFlight(HtmlNode node)
    {
        try
        {
            var cells = node.Elements("td").ToList();

            var ident = WebUtility.HtmlDecode(cells[0].InnerText.Trim());
            var link = cells[0].Element("span").Element("a").Attributes["href"].DeEntitizeValue.Trim();
            var airline = cells[0].Element("span").Attributes["title"].DeEntitizeValue.Trim();
            var type = WebUtility.HtmlDecode(cells[1].InnerText.Trim());
            var fulltype = cells[1].Element("span").Attributes["title"].DeEntitizeValue.Trim();
            var from = WebUtility.HtmlDecode(cells[2].InnerText.Trim());
            var depart = cells.Count >= 4 ? WebUtility.HtmlDecode(cells[3].InnerText.Trim()) : string.Empty;
            var arrive = cells.Count >= 6 ? WebUtility.HtmlDecode(cells[5].InnerText.Trim()) : string.Empty;

            return new LiveFlight(ident, link, airline, type, fulltype, from, depart, arrive);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occured parsing table, ignoring flight");
            return default;
        }
    }
}
