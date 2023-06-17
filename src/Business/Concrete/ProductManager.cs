using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.Utilities.Results;
using Core.Utilities.IoC;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Business.Constants;
using Core.Aspects.Autofac.Validation;
using Entities.Dtos;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        public ProductManager() { }

        [LogAspect(typeof(DatabaseLogger))]
        public IDataResult<List<Product>> Get()
        {
            var _productDal = ServiceTool.ServiceProvider.GetService<IProductDal>();
            var data = _productDal.GetList().ToList();
            return new SuccessDataResult<List<Product>>(data);
        }

        [LogAspect(typeof(DatabaseLogger))]
        public IDataResult<List<Product>> GetByUserId(ProductForUserIdDto dto)
        {
            var _productDal = ServiceTool.ServiceProvider.GetService<IProductDal>();
            var data = _productDal.GetList(x => x.UserId == dto.UserId).ToList();
            return new SuccessDataResult<List<Product>>(data);
        }

        [LogAspect(typeof(DatabaseLogger))]
        public IDataResult<List<Product>> GetById(ProductForIdDto dto)
        {
            var _productDal = ServiceTool.ServiceProvider.GetService<IProductDal>();
            var data = _productDal.GetList(x => (x.UserId == dto.UserId && x.Id == dto.Id)).ToList();
            return new SuccessDataResult<List<Product>>(data);
        }

        [LogAspect(typeof(DatabaseLogger))]
        public IResult Add(Product product)
        {
            var _productDal = ServiceTool.ServiceProvider.GetService<IProductDal>();
            _productDal.Add(product);

            return new SuccessResult(Messages.ProductRegistered);
        }

        [LogAspect(typeof(DatabaseLogger))]
        public IResult Delete(Product product)
        {
            var _productDal = ServiceTool.ServiceProvider.GetService<IProductDal>();
            _productDal.Delete(product);

            return new SuccessResult(Messages.ProductDeleted);
        }

        [LogAspect(typeof(DatabaseLogger))]
        public IResult Update(Product product)
        {
            var _productDal = ServiceTool.ServiceProvider.GetService<IProductDal>();
            _productDal.Update(product);

            return new SuccessResult(Messages.ProductEdited);
        }
    }
}
