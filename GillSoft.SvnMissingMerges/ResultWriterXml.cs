using SharpSvn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace GillSoft.SvnMissingMerges
{
    internal class ResultWriterXml : IResultWriter
    {
        private static string LogFileName = "GillSoft.SvnMissingMerges.Results.xml";

        private static class ElementNames
        {
            public readonly static string CommandLineParameter = "CommandLineParameter";
            public readonly static string DocumentElement = "Results";
            public readonly static string SourceRepository = "SourceRepository";
            public readonly static string TargetRepository = "TargetRepository";

            public readonly static string RevisionInfo = "RevisionInfo";
            public readonly static string ChangedPaths = "Change";
        }

        private static class AttributeNames
        {
            public readonly static string Name = "name";
            public readonly static string Value = "value";
            public readonly static string EndRevision = "endRevision";
            public readonly static string Revision = "revision";
            public readonly static string Author = "author";
            public readonly static string Time = "time";
            public readonly static string NodeKind = "nodeType";
            public readonly static string Action = "action";
            public readonly static string Path = "path";
        }

        private readonly IInputOutputHelper io;
        private readonly XmlDocument doc;
        private readonly XmlElement body;
        public ResultWriterXml(IInputOutputHelper io)
        {
            this.io = io;
            this.doc = new XmlDocument();
            this.body = doc.CreateElement(ElementNames.DocumentElement);

            doc.AppendChild(body);
        }
        void IResultWriter.WriteResults(CommandLineParameters commandLineParameters, List<SvnLogEventArgs> missingRevisions)
        {
            WriteCommandLineParameters(commandLineParameters);

            foreach (var rev in missingRevisions)
            {
                WriteRevisionDetails(rev);
            }
        }

        private void WriteCommandLineParameters(CommandLineParameters commandLineParameters)
        {
            body.AddElement(ElementNames.CommandLineParameter)
                .AddAttribute(AttributeNames.Name, ElementNames.SourceRepository)
                .AddAttribute(AttributeNames.Value, commandLineParameters.SourceRepository);

            body.AddElement(ElementNames.CommandLineParameter)
                .AddAttribute(AttributeNames.Name, ElementNames.TargetRepository)
                .AddAttribute(AttributeNames.Value, commandLineParameters.TargetRepository);

            if (commandLineParameters.EndVersion.HasValue)
            {
                body.AddElement(ElementNames.CommandLineParameter)
                    .AddAttribute(AttributeNames.Name, AttributeNames.EndRevision)
                    .AddAttribute(AttributeNames.Value, commandLineParameters.EndVersion.Value);
            }

        }

        private void WriteRevisionDetails(SvnLogEventArgs revision)
        {
            var elem = body.AddElement(ElementNames.RevisionInfo)
                .AddAttribute(AttributeNames.Revision, revision.Revision)
                .AddAttribute(AttributeNames.Author, revision.Author)
                .AddAttribute(AttributeNames.Time, revision.Time.ToString(CultureInfo.CurrentUICulture))
                .AddAttribute(AttributeNames.Revision, revision.Revision)
                ;

            if (revision.ChangedPaths != null)
            {
                foreach (var item in revision.ChangedPaths)
                {
                    elem.AddElement(ElementNames.ChangedPaths)
                        .AddAttribute(AttributeNames.NodeKind, item.NodeKind)
                        .AddAttribute(AttributeNames.Action, item.Action)
                        .AddAttribute(AttributeNames.Path, item.Path)
                        ;
                }
            }
        }

        void IResultWriter.End()
        {
            var path = Path.GetFullPath(@".\" + LogFileName);
            doc.Save(path);
            io.WriteLine("Results written to: " + path);
#if DEBUG
            Process.Start(path);
#endif
        }

    }
}
