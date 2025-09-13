using System.Text.Json.Serialization;

namespace Movies.Shared.Models;

public class Movie : MovieBaseDomainEntity
{
    public string MovieName { get; private set; } = string.Empty;
    public string DirectorName { get; private set; } = string.Empty;
    public int ReleaseYear { get; private set; }

    #region Ctors
    private Movie() { } // For EF

    [JsonConstructor]
    public Movie(string movieName, string directorName, int releaseYear)
    {
        MovieName = movieName;
        DirectorName = directorName;
        ReleaseYear = releaseYear;
    } 
    #endregion

    public static Movie Create(string movieName, string directorName, int releaseYear) =>
        new()
        {
            MovieName = movieName,
            DirectorName = directorName,
            ReleaseYear = releaseYear
        };

}
