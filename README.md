[![Issue Stats](http://issuestats.com/github/johnazariah/fsm-orleans/badge/issue)](http://issuestats.com/github/johnazariah/fsm-orleans)
[![Issue Stats](http://issuestats.com/github/johnazariah/fsm-orleans/badge/pr)](http://issuestats.com/github/johnazariah/fsm-orleans)

# FSM Orleans

Most interactive applications are informally specified state-machines with intertwined business-logic and state-management.

With the possibility of using virtual actor systems, it is highly desirable to be able to formally specify the states, messages and transitions associated with an application separate from the business logic so that we can better reason about an application's behaviour and correctness.

This project specifies one such DSL, and provides a parser/code-generator to transform the DSL into idiomatic Orleans code, allowing the user to formally specify state-transitions and custom business-logic separately. 
A few analysis tools to reason about the state-machine are also provided.

## Build Status

Mono | .NET
---- | ----
[![Mono CI Build Status](https://img.shields.io/travis/johnazariah/fsm-orleans/master.svg)](https://travis-ci.org/johnazariah/fsm-orleans) | [![.NET Build Status](https://img.shields.io/appveyor/ci/fsgit/ProjectScaffold/master.svg)](https://ci.appveyor.com/project/fsgit/projectscaffold)

## Maintainer(s)

- [@johnazariah](https://github.com/johnazariah)
