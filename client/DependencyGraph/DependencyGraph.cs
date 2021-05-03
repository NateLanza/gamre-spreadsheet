// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities {

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph {

        /// <summary>
        /// Maps nodes to all of their dependents
        /// </summary>
        private Dictionary<string, HashSet<string>> dependents;
        /// <summary>
        /// Maps nodes to all of their dependees
        /// </summary>
        private Dictionary<string, HashSet<string>> dependees;
        /// <summary>
        /// Tracks the number of mappings in this graph
        /// </summary>
        private int size;


        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph() {
            dependees = new Dictionary<string, HashSet<string>>();
            dependents = new Dictionary<string, HashSet<string>>();
            size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size {
            get { return size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s] {
            get {
                return GetLinked(s, false).Count;
            }
        }

        /// <summary>
        /// Gets the HashSet of dependents or dependees for a given key
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <param name="depends">True to return dependents, false to return dependees</param>
        /// <returns>A HashSet containing values, or an empty hashset if key not found</returns>
        private HashSet<string> GetLinked(string key, Boolean depends) {
            //See if the key exists, get its hashset if so
            HashSet<string> values;
            Dictionary<string, HashSet<string>> backer = depends ? dependents : dependees;
            backer.TryGetValue(key, out values);

            // Return based on result of attempt
            if (values == null) {
                return new HashSet<string>();
            } else {
                return values;
            }   
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s) {
            return GetLinked(s, true).Count > 0;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s) {
            return GetLinked(s, false).Count > 0;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s) {
            return GetLinked(s, true);
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s) {
            return GetLinked(s, false);
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t) {
            if (TryAdd(s, t, true) && TryAdd(t, s, false))
                size++;
        }

        /// <summary>
        /// Adds a value to the hashset mapped to a key in one of the dictionaries
        /// </summary>
        /// <param name="key">Key to use</param>
        /// <param name="value">Value to add</param>
        /// <param name="depends">True for dependents dictionary, false for dependees dictionary</param>
        /// <returns>True if value was added, false if the mapping already existed</returns>
        private Boolean TryAdd(string key, string value, Boolean depends) {
            // Get correct dictionary
            Dictionary<string, HashSet<string>> dict = depends ? dependents : dependees;

            // Check if mapping exists
            HashSet<string> values;
            dict.TryGetValue(key, out values);
            if (values != null) {
                return values.Add(value);
            } else {
                values = new HashSet<string>() { value };
                dict.Add(key, values);
                return true;
            }
        }

        /// <summary>
        /// Removes a value from the hashset mapped to a key in one of the dictionaries
        /// </summary>
        /// <param name="key">Key to use</param>
        /// <param name="value">Value to remove</param>
        /// <param name="depends">True for dependents dictionary, false for dependees dictionary</param>
        /// <returns>True if value was remove, false if the mapping doesn't exist</returns>
        private Boolean TryRemove(string key, string value, Boolean depends) {
            // Get correct dictionary
            Dictionary<string, HashSet<string>> dict = depends ? dependents : dependees;

            // Check if mapping exists
            HashSet<string> values;
            dict.TryGetValue(key, out values);
            return values == null ? false : values.Remove(value);
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t) {
            if (TryRemove(s, t, true) && TryRemove(t, s, false))
                size--;
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents) {
            // Get current pairs
            HashSet<string> values;
            dependents.TryGetValue(s, out values);

            // Remove current values
            if (values != null) {
                string[] valuesCopy = new string[values.Count];
                values.CopyTo(valuesCopy);
                foreach (string value in valuesCopy) {
                    this.RemoveDependency(s, value);
                }
            }

            // Add new values
            foreach (string newVal in newDependents) {
                this.AddDependency(s, newVal);
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees) {
            // Get current pairs
            HashSet<string> values;
            dependees.TryGetValue(s, out values);

            // Remove current values
            if (values != null) {
                string[] valuesCopy = new string[values.Count];
                values.CopyTo(valuesCopy);
                foreach (string value in valuesCopy) {
                    this.RemoveDependency(value, s);
                }
            }

            // Add new values
            foreach (string newVal in newDependees) {
                this.AddDependency(newVal, s);
            }
        }

    }

}
