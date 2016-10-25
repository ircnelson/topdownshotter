using UnityEngine;
using System.Collections.Generic;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroups[] Groups;

    private Dictionary<string, AudioClip[]> _groupDictionary = new Dictionary<string, AudioClip[]>();

    void Awake()
    {
        foreach (var soundGroup in Groups)
        {
            _groupDictionary.Add(soundGroup.GroupId, soundGroup.Group);
        }
    }

	public AudioClip GetClipFromName(string name)
    {
        if (_groupDictionary.ContainsKey(name))
        {
            AudioClip[] sounds = _groupDictionary[name];

            return sounds[Random.Range(0, sounds.Length)];
        }

        return null;
    }

    [System.Serializable]
    public class SoundGroups
    {
        public string GroupId;
        public AudioClip[] Group;
    }
}
