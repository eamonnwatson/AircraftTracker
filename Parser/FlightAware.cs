using AircraftTracker.Entities;
using AircraftTracker.Errors;
using AircraftTracker.Interfaces;
using AircraftTracker.Options;
using FluentResults;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net;

namespace AircraftTracker.Parser;
internal class FlightAware(ILogger<FlightAware> logger, IOptions<CheckerOptions> checkerOptions) : IFlightParser
{
    private readonly ILogger<FlightAware> _logger = logger;
    private readonly CheckerOptions _checkerOptions = checkerOptions.Value;

    public async Task<Result<IEnumerable<LiveFlight>>> GetFlightsAsync(string airportICAO, CancellationToken cancellationToken = default)
    {
        try
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(string.Format(_checkerOptions.FlightAwareURL, airportICAO), cancellationToken);

            var enroute = doc.DocumentNode.SelectSingleNode(_checkerOptions.EnrouteTableNode);

            if (enroute is null)
                return Result.Fail(ParserError.TableNotFound);

            var items = GetFlightsFromTable(enroute).ToList();

            if (items.Count == 0)
                return Result.Fail(ParserError.NoFlightsFound);

            return items.Where(f => !string.IsNullOrWhiteSpace(f.Type)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Parsing Error");
            return Result.Fail(ParserError.GeneralError.CausedBy(ex));
        }
    }

    private IEnumerable<LiveFlight> GetFlightsFromTable(HtmlNode enroute)
    {
        var rows = enroute.Elements("tr");

        if (rows is null || !rows.Any())
            return Enumerable.Empty<LiveFlight>();

        return rows.Select(r => ConvertRowToFlight(r)).Where(r => r is not null)!;
    }

    private LiveFlight? ConvertRowToFlight(HtmlNode node)
    {
        try
        {
            var cells = node.Elements("td").ToList();

            if (!cells[0].HasAttributes | !cells[0].HasClass("flight-ident"))
                return default;

            var ident = WebUtility.HtmlDecode(cells[0].InnerText.Trim());
            var link = cells[0].Element("span").Element("a").Attributes["href"].DeEntitizeValue.Trim();
            var airline = cells[0].Element("span").Attributes["title"].DeEntitizeValue.Trim();
            var type = WebUtility.HtmlDecode(cells[1].InnerText.Trim());
            var fulltype = cells[1].Element("span").Attributes["title"].DeEntitizeValue.Trim();
            var from = WebUtility.HtmlDecode(cells[2].InnerText.Trim());
            var depart = cells.Count >= 4 ? WebUtility.HtmlDecode(cells[3].InnerText.Trim()) : string.Empty;
            var arrive = cells.Count >= 6 ? WebUtility.HtmlDecode(cells[5].InnerText.Trim()) : string.Empty;

            return LiveFlight.Create(ident, link, airline, type, fulltype, from, depart, arrive);
        }
        catch (Exception ex)
        {
            ex.Data.Add("InputNodeHtml", node.InnerHtml);
            _logger.LogWarning(ex, "Could not parse flight, ignoring flight");
            return default;
        }
    }
}
