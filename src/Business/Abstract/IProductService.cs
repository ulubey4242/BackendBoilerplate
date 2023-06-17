using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IProductService
    {
        IDataResult<List<Product>> Get();
        IDataResult<List<Product>> GetByUserId(ProductForUserIdDto dto);
        IDataResult<List<Product>> GetById(ProductForIdDto dto);
        IResult Add(Product product);
        IResult Delete(Product product);
        IResult Update(Product product);
    }
}
