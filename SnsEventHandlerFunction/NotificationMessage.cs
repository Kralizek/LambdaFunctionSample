using System;
using System.Text.Json.Serialization;

namespace SnsEventHandlerFunction;

public record NotificationMessage (Guid Id, bool IsSuccess);