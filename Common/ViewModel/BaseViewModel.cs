using MediatR;

namespace Common.ViewModel
{
    public class GenericViewModel<TRequest> where TRequest : IRequest<bool>
    {
        private readonly IMediator _mediator;

        // IMediator 인스턴스를 의존성 주입을 통해 받음
        public GenericViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 제네릭 요청을 처리하고, bool 결과를 반환하는 메소드
        public async Task<bool> HandleRequestAsync(TRequest request)
        {
            return await _mediator.Send(request);
        }
    }
}
