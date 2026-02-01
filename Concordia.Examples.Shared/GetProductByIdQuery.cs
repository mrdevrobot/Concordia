// Example Controller for usage (remains unchanged)
namespace Concordia.Examples.Web.Controllers
{
    /// <summary>
    /// The get product by id query class
    /// </summary>
    /// <seealso cref="IRequest{ProductDto}"/>
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        /// <summary>
        /// Gets or sets the value of the product id
        /// </summary>
        public int ProductId { get; set; }
    }
}