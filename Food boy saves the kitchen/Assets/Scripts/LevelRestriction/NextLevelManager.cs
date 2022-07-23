using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NextLevelManager
{
    /* Stores the order of next level.
     * -1: Special in that it unlocks the next page of levels.
     * Lvl 1, 12, 25, 34 will unlock automatically, being
     * the first levels of their respective pages.
     * 
     * Parameters
     * ----------
     * Key: Level that is finished
     * Value: Levels that will be unlocked due to level finishing
     */
    public static readonly Dictionary<int, List<int>> nextLvl = new()
    {
        {1, new List<int>() {2} },
        {2, new List<int>() {3} },
        {3, new List<int>() {4} },
        {4, new List<int>() {5, 6} },
        {5, new List<int>() {} },
        {6, new List<int>() {7} },
        {7, new List<int>() {8} },
        {8, new List<int>() {9, 10, -1} },
        {9, new List<int>() {} },
        {10, new List<int>() {11} },
        {11, new List<int>() {} },
        {12, new List<int>() {13, 14} },
        {13, new List<int>() {} },
        {14, new List<int>() {15, 16, 17} },
        {15, new List<int>() {} },
        {16, new List<int>() {} },
        {17, new List<int>() {18, 19} },
        {18, new List<int>() {} },
        {19, new List<int>() {20, 21, 22} },
        {20, new List<int>() {} },
        {21, new List<int>() {} },
        {22, new List<int>() {23, 24} },
        {23, new List<int>() {} },
        {24, new List<int>() {-1} },
        {25, new List<int>() {26, 27} },
        {26, new List<int>() {} },
        {27, new List<int>() {28, 29} },
        {28, new List<int>() {} },
        {29, new List<int>() {30} },
        {30, new List<int>() {31} },
        {31, new List<int>() {32} },
        {32, new List<int>() {33, -1} },
        {33, new List<int>() {} },
        {34, new List<int>() {35, 36} },
        {35, new List<int>() {37, 38} },
        {36, new List<int>() {} },
        {37, new List<int>() {} },
        {38, new List<int>() {39, 40} },
        {39, new List<int>() {} },
        {40, new List<int>() {41} },
        {41, new List<int>() {42} },
        {42, new List<int>() {43} },
        {43, new List<int>() {} }
    };

    //Key: Build index, Value: max level.
    public static readonly List<int> levelSelect = new()
        { 2, 3, 4, 5 };
}
