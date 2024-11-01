# HDRICubemapConverter
Can convert between HDRI (equirectangular) and cubemap (horizontally aligned or origami/cross layout) image layouts.
Uses [SimpleImageIO](https://github.com/pgrit/SimpleImageIO), and thus supports multiple image formats, e.g. HDR, EXR, PNG, JPG, BMP and (possibly) more.

```
Usages:
  HDRICubemapConverter hdri [in_cubemap_layout] [filepath] <outpath>
  HDRICubemapConverter [out_cubemap_layout] [filepath] <outpath>

If no outpath is provided, then the output file will use the input filepath + the target layout.
  e.g.: image.hdr -> image_HDRI.hdr

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
