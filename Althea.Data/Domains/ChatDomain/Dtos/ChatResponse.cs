namespace Althea.Data.Domains.ChatDomain;

public record struct ChatResponse(MessageDto Sent, MessageDto? Received, bool IsEnd);
