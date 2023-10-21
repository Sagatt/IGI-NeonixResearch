### Audio Template
Decoding and Playing Project IGI .ILSF Audio Files


### Steps
    Download Required Tools:
        Download the provided file "ilsf_txth.7z" from the post.
        Download foobar2000 from foobar2000 website.

    Extract Files:
        Extract the contents of "ilsf_txth.7z" to a folder on your computer.

    Install foobar2000:
        Install foobar2000 on your computer by following the installation instructions on their website.

    Prepare .ILSF Audio Files:
        Rename the .Wav files from Project IGI to have a .ilsf extension.
		You can use the powershell script to rename all .wav files in a folder in batch.
		"gci -File | Rename-Item -NewName { $_.name -replace "\.(wav|file)$", ".ilsf" }"

    Place Required Files:
        Move the renamed .ilsf files to the same folder where you extracted the contents of "ilsf_txth.7z."

    Rename TXTH Function File:
        Ensure that the TXTH function file is named ".ilsf.txth" (including the dot at the beginning) in the same folder as the renamed .ilsf files.
		
	Install vgmstream
		Download and install vgmstream from the vgmstream git page.
		Foobar2000 should now be able to decode and play the .ILSF audio files from Project IGI.

    Decoding .ILSF Files:
        Open the foobar2000 application on your computer.
        Drag and drop the .ilsf files into the foobar2000 interface.
		Or alternatively onto the vgmstream executable.