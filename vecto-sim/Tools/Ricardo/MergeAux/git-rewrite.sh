#!/bin/bash
# Merge cb11c451816@Current branch from Ricardo's repo with ab66a68@RICARDO_FORK tag from VECTO-2.0 repo (central). 
#
# NOTE: this script has run in small pieces, not all at once.

set -o  errexit


function mark_original_branches {
	git branch auxmerge/ricardo_original   cb11c451816


	## Mark some important commits.
	#	See rewrite_aux_history-tree_filter.sh
	#
	git tag auxmerge/VectoAuxiliariesTB_begin  5d6fafd2e553 -f \
        -m "Renaming of 'VectoAuxiliaries' --> 'VectoAuxiliariesTB'. "
	git tag auxmerge/VectoAuxiliaries_end  80fd13a4b739 -f \
        -m "Last 'valid' modifications of precursor to 'VectoAuxiliariesTB' folder (without 'TB' suffix)."
	git tag auxmerge/VectoAuxiliaries_begin  538e211c6566 -f \
        -m "Initial commit of precursor to 'VectoAuxiliariesTB' folder (without 'TB' suffix)."
	git tag auxmerge/RICARDO_ROOT  57393fe -f
        -m "First commit delivered by Ricardo."
	git tag auxmerge/RICARDO_FORK  ab66a68b7333cd8 -f  \
        -m "According to Dana@Ricardo email on 30/Apr/2015 they FORKed @2a006e6dacf5e7
but JRC assumes they meant the next-one, not-merge commit:
  ab66a68b7333cd8@milestone/2.0.4-beta.2
"
}

function remove_git_backup {
	rm -rf  .git/refs/original # Clear git-backup to continue re-writting.
}

## Eliminate duplicate folders (mostly safe stuff).
# 
# The TMP_DIR may point to some RAMDisk (i.e. for windows see ImDisk).
#
function cleanup_ricardo_sources {
	#git checkout auxmerge/ricardo_original
	#git checkout -b auxmerge/aux1.3-cleanup
	git filter-branch ${TMP_DIR:+-d $TMP_DIR} \
			--prune-empty \
			--tree-filter  "$PWD/cleanup_aux_history.tree_filter.sh"
}


## Move VECTO sources to root & vecto-aux-->AUX.
#
function reroot_vecto_sources {
	#git checkout auxmerge/aux1.3-cleanup
	#git checkout -b auxmerge/aux2.3-reroot_vecto
	git filter-branch ${TMP_DIR:+-d $TMP_DIR} \
			--prune-empty \
			--tree-filter  "$PWD/root_vecto-mv_aux.tree_filter.sh" 
}


#git checkout Current
#mark_original_branches

#remove_git_backup
#cleanup_ricardo_sources 

remove_git_backup
reroot_vecto_sources

##  JOIN the 2 unrelated branches and
#   MANUALLY RESOLVE CONFLICTS
#git branch auxmerge/aux3-rebased_on_fork
#git checkout auxmerge/aux3-rebased_on_fork
#git rebase auxmerge/RICARDO_FORK