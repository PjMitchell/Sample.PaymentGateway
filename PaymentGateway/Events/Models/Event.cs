using System;

namespace PaymentGateway
{
    public abstract record Event()
    {
        public string TrackingId { get; init; } = string.Empty;
        public DateTimeOffset TimeStamp { get; init; }
    };
}
