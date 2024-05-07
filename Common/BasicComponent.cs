using MediatR;
using Microsoft.AspNetCore.Components;

namespace Common
{
    public class BasicComponent<TRequest> : ComponentBase where TRequest : IRequest<bool>
    {
        [Inject]
        protected IMediator Mediator { get; set; }

        protected bool IsProcessing { get; set; }
        protected string ErrorMessage { get; set; }
        protected TRequest Request { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Request = Activator.CreateInstance<TRequest>();
        }

        protected async Task HandleValidSubmit()
        {
            IsProcessing = true;
            ErrorMessage = string.Empty;
            try
            {
                var result = await Mediator.Send(Request);
                if (!result)
                {
                    ErrorMessage = "An error occurred while processing your request.";
                }
                // Optionally, navigate to another page or update UI.
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
