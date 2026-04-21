namespace SafeBite_Backend_H6.API.Interfaces.Services.OCR;

public interface IImageProcessor
{
    byte[] PreprocessForOcr(Stream input);
}
