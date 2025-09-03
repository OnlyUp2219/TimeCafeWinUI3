namespace TimeCafeWinUI3.Application.CQRS.Clients.Get;

public record GetTotalPageQuery() : IRequest<int>;

public class GetTotalPageHandler : IRequestHandler<GetTotalPageQuery, int>
{
    private readonly IClientRepository _repository;

    public GetTotalPageHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(GetTotalPageQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTotalPageAsync();
    }
}
