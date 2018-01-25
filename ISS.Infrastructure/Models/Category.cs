namespace ISS.Infrastructure.Models
{
  public enum Category
  {
    Phone,
    Asp,
    Vocoid,
    Contoid,
    Stopped,
    Silence,
    Glide,
    Fricative,
    Nasal,
    Affricate,
    Checked,
    Voiced,
    Fortis,
    HLike,
    WhistleHack, //Added so we can easily do the rewrite rule:  s* >> [f* | v* | th* | dh*]       transforms to:  s* >> qt* >> [f* | v* | th* | dh*]   Now works as:  s* >> whistlehack         transforms to:  s* >> qt* >> whistlehack"
    LHack
  }
}