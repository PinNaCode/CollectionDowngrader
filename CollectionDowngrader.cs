﻿using Realms;
using Realms.Exceptions;
using CollectionDowngrader.LazerSchema;

namespace CollectionDowngrader
{
    class CollectionDowngrader
    {
        const int LazerSchemaVersion = 26;

        private static int Main(string[] args)
        {
            String realmFile, outputFile;
            FileStream outStream;
            BinaryWriter binWriter;
            Realm db;
            DateTimeOffset lastMod;
            BeatmapCollection? lastModCollection;

            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: CollectionDowngrader <path to osu! (lazer) client.realm> " +
                                        "<output osu! (stable) collection.db path>");

                return 1;
            }

            realmFile = args[0];
            outputFile = args[1];

            if (File.Exists(realmFile)) {
                Console.WriteLine("Found realm file.");
            } else {
                Console.Error.WriteLine("Realm file does not exist, stop.");

                return 1;
            }

            if (File.Exists(outputFile))
            {
                Console.Error.WriteLine("Output file already exists, aborting.");

                return 1;
            }

            RealmConfiguration config = new(realmFile)
            {
                IsReadOnly = true,
                SchemaVersion = LazerSchemaVersion
            };

            config.Schema = new[] {
                typeof(Beatmap),
                typeof(BeatmapCollection),
                typeof(BeatmapDifficulty),
                typeof(BeatmapMetadata),
                typeof(BeatmapSet),
                typeof(BeatmapUserSettings),
                typeof(RealmFile),
                typeof(RealmNamedFileUsage),
                typeof(RealmUser),
                typeof(Ruleset),
                typeof(ModPreset)
            };

            try
            {
                db = Realm.GetInstance(config);
            }
            catch (RealmException re)
            {
                Console.Error.WriteLine($"Error opening database:\n\n{re.Message}");
                if (re.Message.Contains("is less than last set version"))
                {
                    Console.Error.WriteLine("It seemed like the specified osu! (lazer) database is in a new format " +
                                            "which is not compatible with this version of CollectionDowngrader.");
                    Console.Error.WriteLine("\nYou can go check the project page to see if there's a new release, " +
                                            "or file an Issue on GitHub to let me know it needs updating.");
                }

                return 1;
            }

            Console.WriteLine("The specified osu! (lazer) database is loaded successfully.");

            List<BeatmapCollection> collections = db.All<BeatmapCollection>().ToList();
            int collectionCount = collections.Count;

            Console.WriteLine($"Found {collectionCount} collections in the database.");

            try
            {
                outStream = File.Open(outputFile, FileMode.CreateNew, FileAccess.Write);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine($"Cannot create output file for writing: {ioe.Message}");

                return 1;
            }

            Console.WriteLine("Output file is created successfully, now start writing data.");

            binWriter = new BinaryWriter(outStream);

            // find the last modified collection and its modification date
            lastModCollection = collections.MaxBy(i => i.LastModified.Ticks);
            lastMod = lastModCollection is null ? DateTimeOffset.Now : lastModCollection.LastModified;

            binWriter.Write((int) lastMod.Ticks);  // last modification date
            binWriter.Write(collectionCount);  // collection count

            foreach (BeatmapCollection collection in collections)
            {
                binWriter.Write((byte) 0x0b);
                binWriter.Write(collection.Name);  // collection name
                binWriter.Write(collection.BeatmapMD5Hashes.Count);  // beatmap count

                foreach (string hash in collection.BeatmapMD5Hashes)
                {
                    binWriter.Write((byte) 0x0b);
                    binWriter.Write(hash);  // beatmap MD5 hash
                }
            }

            binWriter.Close();
            outStream.Close();

            Console.WriteLine("Everything is OK.");

            return 0;
        }
    }
}