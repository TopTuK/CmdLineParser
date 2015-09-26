# CmdLineParser
Simple Cmd Line Parser for C#

Most applications written today accept command line arguments of some form. Sometimes we only need to parse the command line for file names that should be opened upon starting the applications, and at other times we need to process a large amount of various options controlling the way our application will execute. 
Dealing with command line arguments should be tricky!
.NET does some rudimentary command parsing for you but there are a number of things that you probably need to do that it doesn't do that you probably need to do.

Library is complete enough that it covers most of the common command line parsing tasks:
- Creates and configure optional class.
- Supports options starting with '-' or '/'.
- Supports option and parameter attached in one argument (e.g., -P=123 ).
- Or as an argument pair (e.g. -P 123).
- Basic 'String', 'Integer', and 'Double' parameter options support.
- Intelligently handles different number decimal separators.
- Contains a basic usage (help) message of the available (registered) options.
