# -*- mode: python ; coding: utf-8 -*-

block_cipher = None
face_models = [
('.\\shape_predictor_68_face_landmarks.dat', '.'),
]

a = Analysis(['OpenCV_FaceTracking.py'],
             pathex=['D:\\GameDevlop\\AGI20_Group05_VDungeon\\Assets\\Scripts\\PythonScript'],
             binaries=face_models,
             datas=[],
             hiddenimports=[],
             hookspath=[],
             runtime_hooks=[],
             excludes=[],
             win_no_prefer_redirects=False,
             win_private_assemblies=False,
             cipher=block_cipher,
             noarchive=False)
pyz = PYZ(a.pure, a.zipped_data,
             cipher=block_cipher)
exe = EXE(pyz,
          a.scripts,
          [],
          exclude_binaries=True,
          name='OpenCV_FaceTracking',
          debug=False,
          bootloader_ignore_signals=False,
          strip=False,
          upx=True,
          console=True )
coll = COLLECT(exe,
               a.binaries,
               a.zipfiles,
               a.datas,
               strip=False,
               upx=True,
               upx_exclude=[],
               name='OpenCV_FaceTracking')
