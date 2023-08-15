using AircraftTracker.Pushover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftTracker.Interfaces;
internal interface IPushoverClient
{
    PushResponse Push(string title, string message);
    Task<PushResponse> PushAsync(string title, string message, string? userKey = null, Priority priority = Priority.Normal);
}
