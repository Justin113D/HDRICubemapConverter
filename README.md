# HDRICubemapConverter
Designed to work with .hdr files.
Can convert between HDRI (equilateral) and cubemap (horizontally aligned or origami/cross layout) layouts.

```
Usages:
  HDRICubemapConverter hdri [input cubemap layout] [input cubemap filepath]
  HDRICubemapConverter [output cubemap layout] [input hdri filepath]

Cubemap layouts:
  - line: cube sides in a horizontal line:
            -X+X-Y+Y-Z+Z
  - cube: traditional origami-like layout:
            __+Y____
            -X+Z+X-Z
            __-Y____
```

Requires .NET 8 or newer to run.

Best used in conjunction with [Nvidia Texture Tools Exporter](https://developer.nvidia.com/texture-tools-exporter).
