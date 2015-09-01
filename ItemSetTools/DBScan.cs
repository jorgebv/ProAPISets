using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemSetTools
{
    /// <summary>
    /// This class wraps a pro player and is used in the DBScan algorithm below
    /// </summary>
    public class DBScanProPlayerGame
    {
        public const int NOISE = -1;
        public const int UNCLASSIFIED = 0;

        public ProPlayerGame Game { get; set; }
        public int ClusterId { get; set; }

        public static int Distance(DBScanProPlayerGame g1, DBScanProPlayerGame g2)
        {
            var comparer = new ItemPurchaseTimelineComparer();
            return comparer.DistanceBetween(g1.Game.ItemPurchaseTimeline, g2.Game.ItemPurchaseTimeline);
        }

        public DBScanProPlayerGame(ProPlayerGame game)
        {
            Game = game;
        }
    }

    /// <summary>
    /// C# DBScan implementation taken from
    /// http://www.c-sharpcorner.com/uploadfile/b942f9/implementing-the-dbscan-algorithm-using-C-Sharp/
    /// with minor modifications
    /// </summary>
    public class DBScan
    {
        public List<List<DBScanProPlayerGame>> GetClusters(List<DBScanProPlayerGame> points, int eps, int minPts)
        {
            if (points == null) return null;
            List<List<DBScanProPlayerGame>> clusters = new List<List<DBScanProPlayerGame>>();
            int clusterId = 1;
            for (int i = 0; i < points.Count; i++)
            {
                DBScanProPlayerGame p = points[i];
                if (p.ClusterId == DBScanProPlayerGame.UNCLASSIFIED)
                {
                    if (ExpandCluster(points, p, clusterId, eps, minPts)) clusterId++;
                }
            }
            // sort out points into their clusters, if any
            int maxClusterId = points.OrderBy(p => p.ClusterId).Last().ClusterId;
            if (maxClusterId < 1) return clusters; // no clusters, so list is empty
            for (int i = 0; i < maxClusterId; i++) clusters.Add(new List<DBScanProPlayerGame>());
            foreach (DBScanProPlayerGame p in points)
            {
                if (p.ClusterId > 0) clusters[p.ClusterId - 1].Add(p);
            }
            return clusters;
        }
        private List<DBScanProPlayerGame> GetRegion(List<DBScanProPlayerGame> points, DBScanProPlayerGame p, int eps)
        {
            List<DBScanProPlayerGame> region = new List<DBScanProPlayerGame>();
            for (int i = 0; i < points.Count; i++)
            {
                int dist = DBScanProPlayerGame.Distance(p, points[i]);
                if (dist <= eps) region.Add(points[i]);
            }
            return region;
        }
        private bool ExpandCluster(List<DBScanProPlayerGame> points, DBScanProPlayerGame p, int clusterId, int eps, int minPts)
        {
            List<DBScanProPlayerGame> seeds = GetRegion(points, p, eps);
            if (seeds.Count < minPts) // no core point
            {
                p.ClusterId = DBScanProPlayerGame.NOISE;
                return false;
            }
            else // all points in seeds are density reachable from point 'p'
            {
                for (int i = 0; i < seeds.Count; i++) seeds[i].ClusterId = clusterId;
                seeds.Remove(p);
                while (seeds.Count > 0)
                {
                    DBScanProPlayerGame currentP = seeds[0];
                    List<DBScanProPlayerGame> result = GetRegion(points, currentP, eps);
                    if (result.Count >= minPts)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            DBScanProPlayerGame resultP = result[i];
                            if (resultP.ClusterId == DBScanProPlayerGame.UNCLASSIFIED || resultP.ClusterId == DBScanProPlayerGame.NOISE)
                            {
                                if (resultP.ClusterId == DBScanProPlayerGame.UNCLASSIFIED) seeds.Add(resultP);
                                resultP.ClusterId = clusterId;
                            }
                        }
                    }
                    seeds.Remove(currentP);
                }
                return true;
            }
        }
        // methods below here are 100% mine, no longer from blog

        /// <summary>
        /// Let the center of a cluster be defined as the point that has the lowest cumulative distance
        /// to all other points in the cluster
        /// 
        /// After finding that point, see if there are any other point in the cluster that have distance
        /// 0 but are longer. We will make the longest of these the center. If there is not, the originally
        /// found point is the center
        /// </summary>
        /// <param name="cluster">The cluster to search through</param>
        public DBScanProPlayerGame FindCenterOfCluster(List<DBScanProPlayerGame> cluster)
        {
            // method uses this 2D array to store calculations in an attempt to repeat as few distance
            // calculations as possible
            var distanceArray = new int[cluster.Count, cluster.Count];
            for (int i = 0; i < cluster.Count; i++)
            {
                for (int j = 0; j < cluster.Count; j++)
                {
                    distanceArray[i, j] = -1;
                }
            }

            for (int i = 0; i < cluster.Count; i++)
            {
                for (int j = 0; j < cluster.Count; j++)
                {
                    // if it is != -1, we have already calculated a distance
                    if (distanceArray[i, j] == -1)
                    {
                        if (i == j)
                        {
                            distanceArray[i, j] = 0;
                        }
                        else
                        {
                            distanceArray[i, j] = DBScanProPlayerGame.Distance(cluster[i], cluster[j]);
                        }
                    }
                }
            }

            // distance array is filled. sum each row to find the lowest total distance
            var rowSums = new List<int>();
            for (int i = 0; i < cluster.Count; i++)
            {
                int runningSum = 0;
                for (int j = 0; j < cluster.Count; j++)
                {
                    runningSum += distanceArray[i, j];
                }
                rowSums.Add(runningSum);
            }

            // rowSums[i] now has the sum of all distances from i to every other point
            // the minimum of rowSums is our center point. If there is a tie, we'll take
            // the longer game
            var indexOfMin = -1;
            var currentMin = Int32.MaxValue;
            var lengthOfCurrentCentralGame = -1;
            for (int i = 0; i < rowSums.Count; i++)
            {
                if (rowSums[i] < currentMin)
                {
                    indexOfMin = i;
                    currentMin = rowSums[i];
                    lengthOfCurrentCentralGame = cluster[i].Game.ItemPurchaseTimeline.Count;
                }
                else if (rowSums[i] == currentMin)
                {
                    if (cluster[i].Game.ItemPurchaseTimeline.Count > lengthOfCurrentCentralGame)
                    {
                        indexOfMin = i;
                        currentMin = rowSums[i];
                        lengthOfCurrentCentralGame = cluster[i].Game.ItemPurchaseTimeline.Count;
                    }
                }
                // else this is less central, do nothing
            }

            // we now have the most central game. see if there is a distance 0 game that is longer
            var row = indexOfMin;
            for (int col = 0; col < cluster.Count; col++)
            {
                if (distanceArray[row, col] == 0)
                {
                    // we have found a distance 0 game
                    if (cluster[col].Game.ItemPurchaseTimeline.Count > lengthOfCurrentCentralGame)
                    {
                        // the length of this distance 0 game is longer than what we had currently
                        // settle on. record the index and the length, but keep rolling through
                        // the column to see if we find a better one
                        indexOfMin = col;
                        lengthOfCurrentCentralGame = cluster[col].Game.ItemPurchaseTimeline.Count;
                    }
                }
            }

            return cluster[indexOfMin];
        }
    }
}
