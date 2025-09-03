namespace TimeCafeWinUI3.Application.CQRS.Tariffs.Get;
public record class GetTariffByIdQuery(int tariffId) : IRequest<Tariff>;
public class GetTotalPageHandler : IRequestHandler<GetTotalPageQuery, int>
{
    private readonly ITariffRepository _repository;
    public GetTotalPageHandler(ITariffRepository repository)
    {
        _repository = repository;
    }
    public async Task<int> Handle(GetTotalPageQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTotalPageAsync();
    }
}
