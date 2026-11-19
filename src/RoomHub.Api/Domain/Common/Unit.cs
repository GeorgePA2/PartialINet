namespace RoomHub.Api.Domain.Common;

/// <summary>Success without a value.</summary>
public readonly record struct Unit
{
    public static readonly Unit Value = new();
}
