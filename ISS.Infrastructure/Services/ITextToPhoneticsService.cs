namespace ISS.Infrastructure.Services
{
    public interface ITextToPhoneticsService
    {
        string Parse(string input);
    }
}