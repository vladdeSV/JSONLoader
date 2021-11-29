﻿using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using BepInEx;

using TinyJson;

namespace JLPlugin.Utils
{
    using Data;
    using System;

    public static class JLUtils
    {
        public static void LoadCardsFromFiles()
        {
            foreach ( string file in Directory.EnumerateFiles( Paths.PluginPath, "*.jldr", SearchOption.AllDirectories ).Where( file => !file.EndsWith( "_encounter.jldr" ) ) )
            {
                string filename = file.Substring( file.LastIndexOf( Path.DirectorySeparatorChar ) + 1 );

                Data.CardData card = CreateCardFromJSON( File.ReadAllText( file ) );

                if ( card is null )
                {
                    Plugin.Log.LogWarning( $"Failed to load card { filename }" );

                    continue;
                }
                else
                {
                    if ( !filename.EndsWith( "_card.jldr" ) )
                    {
                        Plugin.Log.LogWarning( $"Card { filename } does not have the '_card' postfix. Cards without this postfix will be unsupported in the future." );
                    }

                }

                if ( card.fieldsToEdit is null )
                {
                    card.GenerateNew();
                    continue;
                }

                card.Edit();
            }
        }

        public static void LoadEncountersFromFiles()
        {
            foreach ( string file in Directory.EnumerateFiles( Paths.PluginPath, "*_encounter.jldr", SearchOption.AllDirectories ) )
            {
                string filename = file.Substring( file.LastIndexOf( Path.DirectorySeparatorChar ) + 1 );

                Data.CustomEncounterData encounter = CreateEncounterFromJSON( File.ReadAllText( file ) );

                if ( encounter is null )
                {
                    Plugin.Log.LogWarning( $"Failed to load encounter { filename }" );

                    continue;
                }

                encounter.GenerateNew();
            }
        }

        public static CardData CreateCardFromJSON( string jsonString )
            => JSONParser.FromJson<CardData>( jsonString );

        public static CustomEncounterData CreateEncounterFromJSON( string jsonString )
            => JSONParser.FromJson<CustomEncounterData>( jsonString );

        public static Texture2D LoadTexture2D( string image )
        {
            string[] imagePaths = Directory.GetFiles( Paths.PluginPath, image, SearchOption.AllDirectories );

            if ( imagePaths.Length == 0 )
            {
                Plugin.Log.LogError( $"{ ErrorUtil.Card } - Couldn't find texture \"{ image }\" to load into { ErrorUtil.Field }" );
                return null;
            }

            if ( imagePaths.Length > 1 )
            {
                Plugin.Log.LogError( $"{ ErrorUtil.Card } - Couldn't load \"{ image }\" into { ErrorUtil.Field }, more than one file with that name found in the plugins folder" );
                return null;
            }

            byte[] imgBytes = File.ReadAllBytes( imagePaths[0] );

            Texture2D texture = new( 2, 2 );

            if ( !texture.LoadImage( imgBytes ) )
            {
                Plugin.Log.LogError( $"{ ErrorUtil.Card } - Couldn't load \"{ image }\" into { ErrorUtil.Field }" );
                return null;
            }

            return texture;

        }
    }
}
