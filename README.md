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
> makemake vscm -DCompiler=gcc
```
loads template `vscm` where the `Compiler` variable is set to `gcc`

### Variables in files
You can use special variables in files using `${}`, if you wan't to escape this you can use `${'${}'}` which will be interpreted as `${}`.

#### Variables that are always present
- `${OutName}`
- `${MainName}`
- `${Extension}`
- `${ }` - expands to nothing

#### If
in the `${}` you can use `,` to create simple if statements

```
${hw,'printf("Hello World");', }
```
if variable `hw` is defined expand to `printf("Hello World");` otherwise expand to variable ` ` (that expands to nothing)
