namespace Althea.Data.Domains.ChatDomain;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Chat, ChatInfoDto>();

        CreateMap<Message, MessageDto>()
            .ForMember(dto => dto.SendTime, s => s.MapFrom(s => s.Audit.CreationTime))
            ;

        CreateMap<ChatOperatorLog, ChatOperatorLogDto>()
            .ForMember(dto => dto.CreationTime, s => s.MapFrom(s => s.Audit.CreationTime));
    }
}
