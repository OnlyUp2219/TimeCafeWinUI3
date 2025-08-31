

namespace TimeCafeWinUI3.Application.CQRS.BillingTypes.GetBillingType;
public record GetBillingTypeByIdQuery(int BillingTypeId) : IRequest<BillingType?>;

public class GetBillingTypeByIdHandler : IRequestHandler<GetBillingTypeByIdQuery, BillingType?>
{
    private readonly IBillingTypeRepository _repository;

    public GetBillingTypeByIdHandler(
        IBillingTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<BillingType?> Handle(GetBillingTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingTypeByIdAsync(request.BillingTypeId);
        return entity;
    }
}