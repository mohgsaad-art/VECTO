# root_vecto-mv_aux.tree_filter.sh: 
#	A git tree-filter script preparing Ricardo's repo to merge with VECTO-2.0 repo (central).
# 
# It is used by the cmd::
#		
#		git filter-branch ${TMP_DIR:+-d $TMP_DIR} --prune-empty  --tree-filter "$PWD/root_vecto-mv_aux.tree_filter.sh" auxmerge/aux-reroot_vecto
#
# Assumes repo has been cleaned-up with cleanup_aux_history.tree_filter.sh
#
# by ankostis, 4-Dec-2015

set -o  errexit


## /vecto-sim-ricardoaeaTB: 
# Move its contents to the root of the project.
#
if [ -d vecto-sim-ricardoaeaTB ]; then
	mv vecto-sim-ricardoaeaTB/ vecto-sim
    grep 'vecto-sim-ricardoaeaTB' -rlZ * | xargs -r0I XXX sed 's/vecto-sim-ricardoaeaTB/vecto-sim/g' -i XXX

    if [ -d Tools ]; then
        mv Tools vecto-sim/.
    fi
fi

## /VectoAuxiliaries-TB: 
# Rename it as `VECTO_AUX`.
#
if [ -d VectoAuxiliaries-TB ]; then
	mv VectoAuxiliaries-TB/ vecto-aux
    grep 'VectoAuxiliaries-TB' -rlZ * | xargs -r0I XXX sed 's/VectoAuxiliaries-TB/vecto-aux/g' -i XXX
fi