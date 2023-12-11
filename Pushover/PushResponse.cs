using System.Runtime.Serialization;

namespace AircraftTracker.Pushover;

[DataContract]
internal class PushResponse
{
    [DataMember(Name = "status")]
    public int Status { get; set; }
    [DataMember(Name = "request")]
    public string Request { get; set; } = string.Empty;
    [DataMember(Name = "errors")]
    public IList<string> Errors { get; set; } = new List<string>();
}