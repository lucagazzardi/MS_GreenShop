using AutoMapper;
using Catalog.API.Model;
using Catalog.API.Model.Others;

namespace Catalog.API.MapperProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductAddEdit>().ReverseMap();
        }
    }
}
