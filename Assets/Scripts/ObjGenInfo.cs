using System.Collections.Generic;

[System.Serializable]
public class ObjGenInfo{
    // public int SEED = 306;
    public List<int> SEEDS;
    public List<float> SEED_Timing;
    public List<int> DiseSeeds;
    public int selSetID = 0;
    public string selSetName = "";
    public string applyStudyName = "";
    public string animClipName = "";

    public List<string> remarks;
    public string command = "";
}
