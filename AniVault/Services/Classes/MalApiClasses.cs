namespace AniVault.Services.Classes;

public class MALAnimeData
{
    public Node node { get; set; }
    public ListStatus? list_status { get; set; }
}

public class ListStatus
{
    public string status { get; set; }
    public int score { get; set; }
    public int num_episodes_watched { get; set; }
    public bool is_rewatching { get; set; }
    public DateTime updated_at { get; set; }
    public string start_date { get; set; }
    public string finish_date { get; set; }
}

public class MainPicture
{
    public string medium { get; set; }
    public string large { get; set; }
}

public class Node
{
    public int id { get; set; }
    public string title { get; set; }
    public MainPicture main_picture { get; set; }
}

public class Paging
{
}

public class Root
{
    public List<MALAnimeData> data { get; set; }
    public Paging paging { get; set; }
}