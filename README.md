## GP2 Slots and Tyre Editor 5 by rremedio

This editor is able to edit the magic data, the tyre data, the physics data and some other data in Microprose Grand Prix 2 (a DOS game from the 90s). This is a version under development, maybe containing a lot of bugs.

The magic data controls various slot relative settings that make racing different from slot to slot, even if you use the same track.

Several value meaning are still unknown. If you discover something, please let me know.

The physics data allows you to control some engine factors, like RPM and Power Curve, and also some aerodinamic and mechanical factor

The tyre data controls the wear and grip for each tyre type. There are 4 tyre types in GP2 (from which you may select one for each track). They are designed in the original game to simulate F1 season 1994. Now you can change it to simulate modern F1 cars (that get better laptimes while fuel is consumpted) or any type of car you want.

### What's new????

>#### Version 5
>- Complete rebuild

### Instalation

Just extract the files to any folder you want and run the .exe.
 
PLEASE BACKUP YOU GP2.EXE BEFORE USING THIS PROGRAM.

The file "original data.zip" contains the original magic data saved in m2d files, physics data save in the file "original.g2p" and tyre data saved in the file "tyres.t2d" (you may load them in the editor).

Old .g2p files (older than Physics Editor 1.1) are not compatible with this version of the software. If you want to use them, export them with GP2 Physics Editor 0.2, import them from the game in GP2 Slots and Tyre Editor 4 beta and save them to a new file (or override the old one),

### Known settings

#### Magic Data
- Tyre Wear - this is the same as the tyre wear you can edit in the Track Editor. If you use GP2Lap, the value used in the game is the one in the track .dat file (edited in TE), if you don't use GP2Lap, the value used will be this one you edit in GP2STE. Bigger values will make the tyres wear faster. This factor affects the human player only!

- Slot Grip - this values an overall grip level for the slot, it works in both Qualifying and Race sessions. It affects only the ccs.

- Qual Grip - this value sets the cc performance for qualifying. Higher values give better performance.

- Race Grip - the same as Qual Grip, but it works for Race sessions.

- Qual Grip 2 and Race Grip 2 - similar to Qual Grip and Race Grip. In original GP2 they have always the same value for qualy and race.

- Qual Timing - changes the time speed, so you can adjust laptimes whitout changing performances. Higher values give slower laptimes.

- Race Timing - the same as Qual Timing, but it works for Race sessions.

- Semi-pro Grip - Adjusts the cc performance in semi-pro level. Value 16384 gives the same performance than ace level, lower values give slower laps, and higher values faster laps. Pro level performance will be between semi-pro and ace. It doesn't affect rookie or amateur levels.

- CC Mistake Rate - higher values will make the ccs miss the corners more times.

- Distance to pit in - distance before the pitlane (in track units) where the ccs will leave the ccline to join the pitlane. It also affects human players.

- Distance to pit out - distance after the pitlane (in track units) where the ccs will join the ccline again.

- Pit in speed - cc speed after he leaves the ccline to join the pitlane.

- Human Fuel - it's the fuel comsumption for human players. Higher values makes the car comsume more fuel. It's not the same as the Fuel Load value in the track editor. That values gives you the amount of fuel loaded in each lap.

- CC Fuel - the same as Human Fuel, but it works for the ccs.

#### Physics Data
- Rev Limiter: The real maximum RPM for the engine.

- Max RPM: The nominal maximum RPM for the engine. The engine will rev higher if the gear ratio is not high enough.

- Power Factor: Affects the power of the engine, changing the top speeds.

- Differential Final Ratios: Adjust the gear ratios. I've had problems on my tests when I've changed it in low power cars. It worked beautifully for the player, but it made te ccs useless...

- Upshift Penalty: When you shift up your gears you lose speed. This factor adjust how much speed you lose. Too high values have a braking effect.

- RPM Lights: Adjust the cockpit lights when you change Max RPM.

- Downforce: The aerodinamical efficiency of the car.

- Braking Force: The braking efficiency of the car.

- Polar Moment of Inertia:
>"POLAR MOMENT OF INERTIA" those of you who got a 'C' in college physics should have a vague idea of what this means. Basically it's how badly the car wants to keep turning or going straight. A car with a lot of weight at the front and/or rear (like a dumbell) will have a large pmi.  Such a car will not go into a spin easily, but will be more likely to keep spinning once it starts.  A car with a small pmi will be less stable, but more nimble, quicker through a slalom or chicane.  A touring car, for example, will have a significantly larger pmi than an F1 car.
>AFFECTS ONLY THE HUMAN CAR!
>>quote from Aubrey Windle's GP2 Physics Editor v1.0 gp2form.txt (I can find a better description)

- Power Curve: It may be the power in a given RPM, but I'm not sure. In GP4 they use it to calculate the torque curve. Thinking in a gap of more or less 435rpm between each value gives good results for me. But I'm not really sure.

- - Asphalt Acceleration 1 and 2 - this values (probably max and min) control the acceleration of the human player's car when the tyres are on the asphalt. Increasing them, you will get faster acceleration with more wheelspin, while deacreasing you will have slower acceleration with less wheelspin.

#### Tyre Data
- Tyre Wear - Higher values will make the tyres wear faster. 

- Tyre grip - Higher values give more grip. Do not use values lower than 16384. It will give strange results.

#### Overall CC Grip Level
These factors work in a similar way than magic data's Qual Grip and Race Grip, but they affect cc performance in all slots at the same time. GP2 default is 16588 for both qualy and race. 

#### Human Grip
The Human Grip changes the driving grip for the human player. Default is 16384. It will change the grip for all tracks. If you are a track maker, you will notice that its effetc is similar do using the pair of commands 0xdd 0xd2 together on a track.

#### GP2Limit (track size)
The Track Size Limit changes the same factor than the GP2Limit program by Philip Woodward. My aim is to add a simpler Windows tool to do the same job. But notice that, at this point, GP2Limit is more reliable.

#### Simultaneous Cars Limit
This factor changes the limit for multiple cars on track at non-race sessions. GP2 uses several values to calculate which ccs are allowed to be on track at each moment and that calculation depends on the fuel laps loaded for that session (which you can adjust with GP2Edit). This value sets a limit, so no matter how much fuel laps are set at GP2Edit, the number or cars on track won't be bigger than this factor.

#### Session Duration
Allows you to change Practice and Qualifying duration. You can extend then to beyond the usual GP2 limits. If you set Qualifying duration to more than 63 minutes, the pit display will display 63, but won't change the display until there are only 62 minutes left.

#### Pit Stop Patch
- Time Factor 1 and 2: these factor change the amount of time spent into the pits when refueling.

- Remove Refueling (patch/unpatch): this will make all cars start with fuel for the full race and when making pit stops, the ccs won't refuel. It doesn't affect pit stop duration.

#### Saved Games
Edit car setups and pitstop strategies from race saved games.

#### Track Manager
IMPORTANT: THIS FEATURE REQUIRES GP2LAP.

This will allow you to edit the tracks loaded by GP2Lap into the game. It is the same thing as manually editing gp2lap.cfg.

In the next version, we will hopefuly have a way to store season files for more convenience.

All track files must be inside your GP2 folder (using the Circuit folder, or subfolders inside it, is recommended for keeping things organized. Using the jam folder is not a bad idea too).

Click the Import button to load the configuration from your gp2lap.cfg file. Clicking on Slot N will let you chose another track for that slot. Clicking on a track name will display soe info about that track. Once you are happy, Export and it will update your gp2lap.cfg.

### Disclaimer
It works good on my GP2. :D

You may find me at the GP2 Forum (http://www.grandprix2.de/forum). I recommend you to make contact by the forum so other GP2 enthusiasts might join the discussion.
