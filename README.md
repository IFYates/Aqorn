# Aqorn
Generate complex SQL inserts from a human-readable data model.

:::mermaid
flowchart LR
    O>Options] --> P
    P[/SQL Writer/] --> S[[SQL]]
    M1[Spec Models] -->|Convert| D
    M1 ---> E1[[Errors]]
    M2 ---> E2[[Errors]]
    D[Dataset] --> P
    M2[Data Models] -->|Add| D
    M3[Data Models] -->|Add| D
    F1>spec file] -->|Parse| M1
    F2>data file] -->|Parse| M2
    F3>data file] -->|Parse| M3
    M3 ---> E3[[Errors]]
:::