// Example Controller for usage (remains unchanged)
namespace Concordia.Examples.Web.Controllers
{
    /// <summary>
    /// The hello command class
    /// </summary>
    /// <seealso cref="IRequest{string}"/>
    public class HelloCommand : IRequest<string>
    {
        /// <summary>
        /// Gets or sets the value of the name
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}