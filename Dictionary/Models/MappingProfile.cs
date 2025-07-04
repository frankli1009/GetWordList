using System;
using AutoMapper;

namespace Dictionary.Models
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<ServiceFile, ServiceFileWithoutData>();
		}
	}
}

