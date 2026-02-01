// Example Controller for usage (remains unchanged)
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Concordia.Examples.Web.Controllers
{
    /// <summary>
    /// The get product by id query handler class
    /// </summary>
    /// <seealso cref="IRequestHandler{GetProductByIdQuery, ProductDto}"/>
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        /// <summary>
        /// Handles the request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task containing the product dto</returns>
        public Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Handling GetProductByIdQuery for ProductId: {request.ProductId}");
            var product = new ProductDto { Id = request.ProductId, Name = $"Product {request.ProductId}", Price = 10.50m };
            return Task.FromResult(product);
        }
    }
}