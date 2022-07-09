# MakeMake

Command line utility for creating and loading project templates.

## Usage
```shell
> makemake -h
```
shows help

```shell
> makemake vscm
```
loads template with name vscm

```shell
> makemake -n vscm
```
creates new template from the files in the cwd with the name vscm

```shell
> makemake vscm -DCompiler=gcc -Dhw
```
loads template `vscm` where the `Compiler` variable is set to `gcc` and the `hw` variable is set to nothing

### Variables in files
Filenames that start with `'` will have their names and contents expanded.

You can use special variables in files using `${}`, you can write literal strings with `${'${\'}'}` which will expand to `${'}`. In file and folder names, the `$` and `\` symbols are changed for `_` and the `{` and `}` symbols are changed for `(` and `)` respectively (`'_('_'__()').c` will expand to `'_().c`).

File in the base of the template with name `'.json` is special. In this file you can specifiy template-specific variables.

#### Variables that are always defined
- `Name` - used for the name of the aplication
- `MainName` - used for the name of the file with main
- `Extension` - used as the extension of executable files
- ` `(space) - expands to nothing

#### Variables often used by templates
- `Compiler` - used for the default compiler
- `DebugFlags` - used for the flags for compiler while debugging
- `ReleaseFlags` - used for the flags for compiler for release

#### If
in the `${}` you can use `,` to create simple if statements

```
${hw,'printf("Hello World");', }
```
if variable `hw` is defined expand to `printf("Hello World");` otherwise expand to variable ` `(space) (that expands to nothing)
