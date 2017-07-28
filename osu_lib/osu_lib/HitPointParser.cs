using System;
using System.Collections.Generic;

using osu_lib.Structures;
using osu_lib.BeatmapInfo;
using osu_lib.HitObjects;

namespace osu_lib
{
    //Parses a .osu file and gets a list of HitPoints from the Hit Objects section
    public class HitPointParser
    {
        Beatmap map;
        HitPoint[] hitpoints;

        public HitPointParser(string filename)
        {
            map = new Beatmap(filename);

            if(map.GetTag("general", "mode") == "3")
                throw new ArgumentException("Error, mania mode parsing not supported");

            List<HitPoint> hitpointbuffer = new List<HitPoint>();
            string[] hitobjectids = map.GetSection("hitobjects");

            for(int i = 0; i < hitobjectids.Length; i++)
            {
                try
                {
                    GenericHitObject hobject = new GenericHitObject(hitobjectids[i], map);

                    int[] positions = hobject.GetHitLocations();
                    int[] times = hobject.GetHitTimes();

                    if(positions.Length != times.Length)
                        throw new Exception("Error: position and times array mismatched in size\n" +
                                            "positions.Length: " + positions.Length + "\n" +
                                            "times.Length: " + times.Length + "\n" +
                                            "index: " + i);

                    //Add a new HitPoint for every hit position and time
                    for(int j = 0; j < positions.Length; j++)
                    {
                        HitPoint point = new HitPoint(positions[j], times[j], 0, 0);
                        hitpointbuffer.Add(point);
                    }
                }
                catch (Exception e)
                {
                    //This is zero-indexed, so the first object is object=0
                    throw new Exception(e.Message + "\nobject=" + i + "\n" + GetMetadata("title") + ": " + GetMetadata("version"), e);
                }
            }

            hitpoints = hitpointbuffer.ToArray();
        }

        public HitPoint[] GetHitPoints()
        {
            return hitpoints;
        }

        //For convenience, HitPointParser allows you to access the osu file's metadata
        public string GetMetadata(string tag)
        {
            string result = map.GetTag("metadata", tag);
            //In the .osu file definition, "Artist" is returned if "ArtistUnicode" doesn't exist
            if(result == null && tag.ToUpper() == "ARTISTUNICODE")
            {
                result = map.GetTag("metadata", "artist");
            }

            return result;
        }

        //Another convenience method
        public string GetMode()
        {
            return map.GetTag("general", "mode");
        }
    }
}
