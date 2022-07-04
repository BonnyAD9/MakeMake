# MakeMake

Command line utility for cearing and loading project templates.

## Usage
```shell
makemake -h
```
shows help

```shell
> makemake vscm
```
loads template with name vscm

```shell
> makemake
```
finds a C file in this folder and creates a Makefile for it

```shell
> makemake -n vscm
```
creates new template from the files in the cwd with the name vscm
