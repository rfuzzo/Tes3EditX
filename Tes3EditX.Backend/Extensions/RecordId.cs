namespace Tes3EditX.Backend.Extensions;

//public readonly struct RecordId
//{
//    public RecordId(string tag, string editorId)
//    {
//        Tag = tag;
//        EditorId = editorId;
//    }

//    public string Tag { get; }
//    public string EditorId { get; }
//}

public record RecordId(string Tag, string EditorId);
