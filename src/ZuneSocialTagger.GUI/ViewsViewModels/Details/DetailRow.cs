using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.GUI.Models;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Details
{
    public class DetailRow
    {
        public DetailRow()
        {
            AvailableZuneTracks = new List<TrackWithTrackNum>();
        }

        public TrackWithTrackNum SongDetails { get; set; }
        public List<TrackWithTrackNum> AvailableZuneTracks { get; set; }
        public TrackWithTrackNum SelectedSong { get; set; }

        /// <summary>
        /// Matches song titles, only matches if the titles are exactly the same, needs extending
        /// </summary>
        /// <returns></returns>
        public void MatchTheSelectedSongToTheAvailableSongs()
        {
            if (SongDetails == null)
                return;

            if (string.IsNullOrEmpty(SongDetails.TrackTitle))
                return;

            if (AvailableZuneTracks.Count() == 0)
                return;

            //this matches album songs to zune website songs in the details view
            //Hold Your Colour ---- hold your colour (Album) = MATCH
            //Hold your colour ---- hold your face = NO MATCH
            this.SelectedSong = AvailableZuneTracks.Where(song => song.TrackTitle.ToLower()
                    .Contains(SongDetails.TrackTitle.ToLower()))
                    .FirstOrDefault();


            //fallback to word matching if we can't match the title exactly
            var results = new Dictionary<TrackWithTrackNum, int>();
            foreach (var song in AvailableZuneTracks)
            {
                int weightedMatches = MatchWordsWeightedByPosition(song.TrackTitle.ToLower(), SongDetails.TrackTitle.ToLower());
                results.Add(song, weightedMatches);
            }

            //select the song with the most words matching
            var mostMatches = results.OrderByDescending(val => val.Value);
            if (mostMatches.First().Value != 0)
            {
                this.SelectedSong = mostMatches.First().Key;
            }
            
        }

        private int MatchWordsWeightedByPosition(string a, string b)
        {
            var aWords = GetWords(a);
            var bWords = GetWords(b).ToList();

            var intersection = aWords.Intersect(bWords);

            //the closer we are to the start of the string the higher the weighting the word gets
            int weighting = 0;
            foreach (var word in intersection)
            {
                int idx = bWords.IndexOf(word);
                if (idx == 0)
                    weighting += 5;
                if (idx == 1)
                    weighting += 4;
                if (idx == 2)
                    weighting += 3;
                if (idx == 3)
                    weighting += 2;
                if (idx == 4)
                    weighting += 1;
            }

            return intersection.Count() + weighting;
        }

        private string[] GetWords(string @string)
        {
            List<string> result = new List<string>();

            MatchCollection aMatches = Regex.Matches(@string, @"\w(?<!\d)[\w'-]*");
            foreach (Match match in aMatches)
            {
                if (match.Success)
                {
                    result.Add(match.Value);
                }
            }

            return result.ToArray();
        }
    }
}