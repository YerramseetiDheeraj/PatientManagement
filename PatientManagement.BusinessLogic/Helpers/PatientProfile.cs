using AutoMapper;
using PatientManagement.Data;
using PatientManagement.Models;

namespace PatientManagement.Mapping
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientCreateModel, Patient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)));

            CreateMap<PatientUpdateModel, Patient>()
                .ForMember(dest => dest.FirstName, opt => opt.Condition((src, dest, srcMember, context) =>
                    !string.IsNullOrWhiteSpace(src.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.Condition((src, dest, srcMember, context) =>
                    !string.IsNullOrWhiteSpace(src.LastName)))
                .ForMember(dest => dest.DateOfBirth, opt => opt.Condition((src, dest, srcMember, context) =>
                    src.DateOfBirth != default))
                .ForMember(dest => dest.Gender, opt => opt.Condition((src, dest, srcMember, context) =>
                    !string.IsNullOrWhiteSpace(src.Gender)))
                .ForMember(dest => dest.ContactNumber, opt => opt.Condition((src, dest, srcMember, context) =>
                    !string.IsNullOrWhiteSpace(src.ContactNumber)))
                .ForMember(dest => dest.Weight, opt => opt.Condition((src, dest, srcMember, context) =>
                    src.Weight != default(decimal)))
                .ForMember(dest => dest.Height, opt => opt.Condition((src, dest, srcMember, context) =>
                    src.Height != default(decimal)))
                .ForMember(dest => dest.Email, opt => opt.Condition((src, dest, srcMember, context) =>
                    !string.IsNullOrWhiteSpace(src.Email)))
                .ForMember(dest => dest.Address, opt => opt.Condition((src, dest, srcMember, context) =>
                    !string.IsNullOrWhiteSpace(src.Address)))
                .ForMember(dest => dest.MedicalComments, opt => opt.Condition((src, dest, srcMember, context) =>
                    !string.IsNullOrWhiteSpace(src.MedicalComments)))
                .ForMember(dest => dest.AnyMedicalTakings, opt => opt.Condition((src, dest, srcMember, context) =>
                    true))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
