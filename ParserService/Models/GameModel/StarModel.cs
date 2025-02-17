namespace ParserService.Models.GameModel
{   
  
    public class RootobjectStar
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public Conceptretrieve conceptRetrieve { get; set; }
    }

    public class Conceptretrieve
    {
        public string __typename { get; set; }
        public Defaultproduct defaultProduct { get; set; }
        public string id { get; set; }
        public Releasedate releaseDate { get; set; }
    }

    public class Defaultproduct
    {
        public string __typename { get; set; }
        public Concept concept { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Starrating starRating { get; set; }
        public string topCategory { get; set; }
        public Webcta[] webctas { get; set; }
    }

    public class Concept
    {
        public string __typename { get; set; }
        public string id { get; set; }
    }

    public class Starrating
    {
        public string __typename { get; set; }
        public float averageRating { get; set; }
        public string averageRatingForDisplay { get; set; }
        public Ratingsdistribution[] ratingsDistribution { get; set; }
        public int totalRatingsCount { get; set; }
    }

    public class Ratingsdistribution
    {
        public string __typename { get; set; }
        public string percentage { get; set; }
        public int percentageRaw { get; set; }
        public int rating { get; set; }
    }

    public class Webcta
    {
        public string __typename { get; set; }
        public Action action { get; set; }
        public Meta meta { get; set; }
        public string type { get; set; }
    }

    public class Action
    {
        public string __typename { get; set; }
        public Param[] param { get; set; }
        public string type { get; set; }
    }

    public class Param
    {
        public string __typename { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Meta
    {
        public string __typename { get; set; }
        public bool preOrder { get; set; }
    }

    public class Releasedate
    {
        public string __typename { get; set; }
        public string type { get; set; }
    }

}
