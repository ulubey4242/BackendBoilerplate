using Business.Abstract;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using WebAPI.Attributes;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [JWTAuth]
    public class ProductController : Controller
    {
        private IProductService _product;

        public ProductController(IProductService Product)
        {
            _product = Product;
        }

        [HttpGet("get")]
        [SwaggerOperation(Summary = "GetAll")]
        public IActionResult Get()
        {
            var result = _product.Get();
            return Ok(result);
        }

        [HttpGet("getbyuserid")]
        [SwaggerOperation(Summary = "GetByUserId")]
        public IActionResult GetByUserId(ProductForUserIdDto dto)
        {
            var result = _product.GetByUserId(dto);
            return Ok(result);
        }

        [HttpGet("getbyid")]
        [SwaggerOperation(Summary = "GetById")]
        public IActionResult GetById(ProductForIdDto dto)
        {
            var result = _product.GetById(dto);
            return Ok(result);
        }

        [HttpPost("add")]
        [SwaggerOperation(Summary = "Add")]
        public IActionResult Add(Product product)
        {
            var result = _product.Add(product);
            return Ok(result);
        }

        [HttpPost("delete")]
        [SwaggerOperation(Summary = "Delete")]
        public IActionResult Delete(Product product)
        {
            var result = _product.Delete(product);
            return Ok(result);
        }

        [HttpPost("update")]
        [SwaggerOperation(Summary = "Update")]
        public IActionResult Update(Product product)
        {
            var result = _product.Update(product);
            return Ok(result);
        }
    }
}
