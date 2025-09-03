namespace TimeCafeWinUI3.Application.CQRS.Tariffs.Get;

public record class GetTariffsPageQuery(int pageNumber, int pageSize) : IRequest<IEnumerable<Tariff>>;
public class GetTariffByIdHandler : IRequestHandler<GetTariffByIdQuery, Tariff>
{
    private readonly ITariffRepository _repository;
    public GetTariffByIdHandler(ITariffRepository repository)
    {
        _repository = repository;
    }
    public async Task<Tariff> Handle(GetTariffByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTariffByIdAsync(request.tariffId);
    }
}
