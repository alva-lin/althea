namespace Althea.Data.Domains.ChatDomain;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Chat, ChatInfoDto>()
            .ForMember(dto => dto.Messages, s => s.MapFrom(source => source.Messages.ToArray()))
            ;

        CreateMap<Message, MessageDto>()
            .ForMember(dto => dto.SendTime, s => s.MapFrom(source => source.Audit.CreationTime))
            .ForMember(dto => dto.ChatId, s => s.MapFrom(source => source.Chat.Id))
            .ForMember(dto => dto.VoiceId, s => s.MapFrom(source => source.VoiceId))
            .ForMember(dto => dto.PrevMessageId, s => s.MapFrom((source, _) => source.PrevMessage?.Id))
            .ForMember(dto => dto.NextMessageIds,
                s => s.MapFrom(source => source.NextMessages.Select(m => m.Id).ToArray()))
            ;
    }
}