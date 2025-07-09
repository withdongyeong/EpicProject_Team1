using UnityEngine.Localization;

public class LocalizeManager : Singleton<LocalizeManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void TileCategoryTextChange(string tileCategory, System.Action<string> callback)
    {
        LocalizedString localizedString = new LocalizedString
        {
            TableReference = "EpicProject_Table",
            TableEntryReference = "Tile_TileCategoty_" + tileCategory
        };

        localizedString.StringChanged += (value) => callback?.Invoke(value);
    }
}
