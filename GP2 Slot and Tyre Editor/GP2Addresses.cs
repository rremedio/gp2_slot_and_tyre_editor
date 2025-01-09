﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP2_Slot_and_Tyre_Editor
{
    public class GP2Addresses
    {
        public static long[] SlotAddresses = new long[]
            {
                //slot 1
                1280584,1280616,1280680,1280744,1280840,1280872,1280904,1280936,1280976,1280978,1281040,1281072,
                1281134,1281230,1281232,1281326,1281328,1281422,1281454,1281456,1281458,1281550,1281582,1281614,
                //slot 2
                1280586,1280620,1280684,1280748,1280842,1280874,1280906,1280938,1280980,1280982,1281042,1281074,
                1281136,1281236,1281238,1281332,1281334,1281424,1281460,1281462,1281464,1281552,1281584,1281618,
                //slot 3
                1280588,1280624,1280688,1280752,1280844,1280876,1280908,1280940,1280984,1280986,1281044,1281076,
                1281138,1281242,1281244,1281338,1281340,1281426,1281466,1281468,1281470,1281554,1281586,1281622,
                //slot 4 etc
                1280590,1280628,1280692,1280756,1280846,1280878,1280910,1280942,1280988,1280990,1281046,1281078,
                1281140,1281248,1281250,1281344,1281346,1281428,1281472,1281474,1281476,1281556,1281588,1281626,
                1280592,1280632,1280696,1280760,1280848,1280880,1280912,1280944,1280992,1280994,1281048,1281080,
                1281142,1281254,1281256,1281350,1281352,1281430,1281478,1281480,1281482,1281558,1281590,1281630,
                1280594,1280636,1280700,1280764,1280850,1280882,1280914,1280946,1280996,1280998,1281050,1281082,
                1281144,1281260,1281262,1281356,1281358,1281432,1281484,1281486,1281488,1281560,1281592,1281634,
                1280596,1280640,1280704,1280768,1280852,1280884,1280916,1280948,1281000,1281002,1281052,1281084,
                1281146,1281266,1281268,1281362,1281364,1281434,1281490,1281492,1281494,1281562,1281594,1281638,
                1280598,1280644,1280708,1280772,1280854,1280886,1280918,1280950,1281004,1281006,1281054,1281086,
                1281148,1281272,1281274,1281368,1281370,1281436,1281496,1281498,1281500,1281564,1281596,1281642,
                1280600,1280648,1280712,1280776,1280856,1280888,1280920,1280952,1281008,1281010,1281056,1281088,
                1281150,1281278,1281280,1281374,1281376,1281438,1281502,1281504,1281506,1281566,1281598,1281646,
                1280602,1280652,1280716,1280780,1280858,1280890,1280922,1280954,1281012,1281014,1281058,1281090,
                1281152,1281284,1281286,1281380,1281382,1281440,1281508,1281510,1281512,1281568,1281600,1281650,
                1280604,1280656,1280720,1280784,1280860,1280892,1280924,1280956,1281016,1281018,1281060,1281092,
                1281154,1281290,1281292,1281386,1281388,1281442,1281514,1281516,1281518,1281570,1281602,1281654,
                1280606,1280660,1280724,1280788,1280862,1280894,1280926,1280958,1281020,1281022,1281062,1281094,
                1281156,1281296,1281298,1281392,1281394,1281444,1281520,1281522,1281524,1281572,1281604,1281658,
                1280608,1280664,1280728,1280792,1280864,1280896,1280928,1280960,1281024,1281026,1281064,1281096,
                1281158,1281302,1281304,1281398,1281400,1281446,1281526,1281528,1281530,1281574,1281606,1281662,
                1280610,1280668,1280732,1280796,1280866,1280898,1280930,1280962,1281028,1281030,1281066,1281098,
                1281160,1281308,1281310,1281404,1281406,1281448,1281532,1281534,1281536,1281576,1281608,1281666,
                1280612,1280672,1280736,1280800,1280868,1280900,1280932,1280964,1281032,1281034,1281068,1281100,
                1281162,1281314,1281316,1281410,1281412,1281450,1281538,1281540,1281542,1281578,1281610,1281670,
                1280614,1280676,1280740,1280804,1280870,1280902,1280934,1280966,1281036,1281038,1281070,1281102,
                1281164,1281320,1281322,1281416,1281418,1281452,1281544,1281546,1281548,1281580,1281612,1281674
            };

        public static long[] PhysicsAddresses = new long[]
        {
            1282660,1282672,1282820,1284436,1284440,1284444,1284448,1282580,1282584,1282241,1282245,
            1282824,1282826,1282828,1282830,1282832,1282834,1282836,1282838,1282840,1282842,1282844,
            1282846,1282848,1282850,1282852,1282854,1282856,1282858,1282860,1282862,1282864,1282866,
            1282868,1282870,1282872,1282874,1282876,1282878,1282880,1282882,1282884,1282886,1282888,
            1282890,1282892,1282894,1282300,1279570,1282276,1281904,1281924
        };

        public static long[] TireAddresses = new long[]
        {
            1282099,1282103,1282107,1282111,1282114,1282118,1282122,1282126
        };
        public static long[] CCGripLevelAddresses = new long[]
        {
            1280968,1280972
        };
        public static long HumanGripLevelAddress = 1281864;
        public static long SimultaneousCCAddress = 0xAC1ED;
        public static long TrackSizeAddress = 1097805;
        public static long[] RefuelPatchAddresses = new long[] { 0xA47AD, 0xA487E };
        public static long[] SessionDurationAddresses = new long[] { 6472, 6474 };
        public static long[] RefuelTimeAddresses = new long[] { 0xA25FA, 0xA24F9 };
    }
}
