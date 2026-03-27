# -*- coding: utf-8 -*-
"""
Created on Wed Jul  3 10:23:50 2024

@author: alixgui
"""
import os
import json
import subprocess
import pandas as pd
import copy
from lxml import etree
import re

def get_vecto_git_root(path: str) -> str | None:
    """Walk up from path until a git repository root is found.

    Uses 'git -C <path> rev-parse --show-toplevel' which automatically
    traverses parent directories. Returns the repo root path, or None
    if no git repository is found.
    """
    result = subprocess.run(
        f'git -C "{path}" rev-parse --show-toplevel',
        stdout=subprocess.PIPE, stderr=subprocess.DEVNULL, shell=True
    )
    if result.returncode != 0:
        return None
    return result.stdout.decode('utf-8').strip()


def get_environment():
    try:
        return os.environ['CI'] or False
    except:
        return False

def get_dirnames():
        return []

IS_CI = get_environment()
vecto_dir_names = r'ci-output' if IS_CI else get_dirnames()

# 0: script name, depends on invocation i.e.:
#       `python script.py`    -> argv[0] == script
#       `python -c script.py` -> argv[0] == -c
# 1: first terminal argument, i.e. vecto path.
MIN_ARG_COUNT = 2

def json_cor_and_load(file):
    with open(file,encoding='utf-8-sig') as f:
        s = f.read()
        s = s.replace('\t','')
        s = s.replace('\n','')
        s = s.replace(',}','}')
        # s = s.replace(',]',']')
        s = re.sub(',[\n ]+]', ']', s)
        # s = s.replace('],  }',']  }')
        # s = s.replace('},    ]','}    ]')
        s = re.sub('}[\n, ]+]', '}  ]', s)
        s = re.sub('][\n, ]+}', ']  }', s)

        if s.count('}')>s.count('{'):
            idx = s.rfind('}')
            s = s[0:idx]

        data = json.loads(s)
        return data


def check_format(file):
    if file.lower().endswith('.xml'):
        return 'xml'

    # filedata =  open(file,encoding='utf-8-sig')
    try:
        # json.load(filedata)
        json_cor_and_load(file)
        return 'json'
    except ValueError:
        return 'csv'

def load_data(inputs_dic,dico,index=None,rootdir='',prefix=''):
    if index is None:
        idx_fld=''
    else:
        idx_fld='_'+str(index)

    idx_test=0

    for fld,val in dico.items():
        # if val is a file, load it ...

        if (isinstance(val,str) and os.path.isfile(os.path.join(inputs_dic['root'],rootdir,val))):

            fld_file = prefix + fld + idx_fld

            file_fpath = os.path.join(inputs_dic['root'],rootdir,val)
            file_locpath = os.path.normpath(os.path.join(rootdir,val))
            # print(file_fpath)

            if 'subfiles' not in inputs_dic.keys():
                inputs_dic['subfiles']={}

            fld_file = prefix + fld + idx_fld

            while fld_file in inputs_dic['subfiles'].keys():
                idx_test=idx_test+1
                idx_fld = '_'+str(idx_test)
                fld_file = prefix + fld + idx_fld

            inputs_dic['subfiles'][fld_file]={}

            inputs_dic['subfiles'][fld_file]['file'] = r'{}'.format(file_locpath) # stored as a raw string
            # inputs_dic['subfiles'][fld + idx_fld]['root'] = os.path.dirname(val)

            rootdir2 = os.path.dirname(val)
            rootdir2 = os.path.join(rootdir,rootdir2)

            file_format = check_format(file_fpath)
            inputs_dic['subfiles'][fld_file]['type'] = file_format


            if file_format=='json':
                # f = open(file_fpath,encoding='utf-8-sig')
                # data = json.load(f)
                data = json_cor_and_load(file_fpath)
                inputs_dic['subfiles'][fld_file]['data'] = data
                load_data(inputs_dic,data,rootdir=rootdir2)
            elif file_format=='xml':
                data = etree.parse(file_fpath)
                inputs_dic['subfiles'][fld_file]['data'] = data
            else:

                try:
                    inputs_dic['subfiles'][fld_file]['data'] = pd.read_csv(file_fpath,encoding='utf-8-sig')
                except:
                    inputs_dic['subfiles'][fld_file]['data'] = pd.read_csv(file_fpath,encoding='unicode_escape')



        if isinstance(val,dict):
            if '1' in val.keys():
                load_data(inputs_dic,val,rootdir=rootdir,prefix=fld+'_')
            else:
                load_data(inputs_dic,val,rootdir=rootdir)

        if isinstance(val,list):
            # print(100*'=')
            # print('len: ', len(val))
            # print(100*'=')
            for idx,elt in enumerate(val):
                # print(elt)
                # print('--> ', str(isinstance(elt,dict)))
                if isinstance(elt,dict):
                    if len(val)>1:
                        load_data(inputs_dic,elt,index=idx,rootdir=rootdir)
                    else:
                        load_data(inputs_dic,elt,rootdir=rootdir)
                if (isinstance(elt,str) and os.path.isfile(os.path.join(inputs_dic['root'],rootdir,elt))):

                    file_fpath = os.path.join(inputs_dic['root'],rootdir,elt)
                    file_locpath = os.path.normpath(os.path.join(rootdir,elt))
                    # print(file_fpath)

                    if 'subfiles' not in inputs_dic.keys():
                        inputs_dic['subfiles']={}

                    idx_test = idx

                    idx_fld = '_'+str(idx)
                    fld_file = prefix + fld + idx_fld

                    while fld_file in inputs_dic['subfiles'].keys():
                        idx_test=idx_test+1
                        idx_fld = '_'+str(idx_test)
                        fld_file = prefix + fld + idx_fld

                    inputs_dic['subfiles'][fld_file]={}

                    inputs_dic['subfiles'][fld_file]['file'] = r'{}'.format(file_locpath) # stored as a raw string
                    # inputs_dic['subfiles'][fld + idx_fld]['root'] = os.path.dirname(val)

                    rootdir2 = os.path.dirname(elt)

                    file_format = check_format(file_fpath)
                    inputs_dic['subfiles'][fld_file]['type'] = file_format


                    if file_format=='json':
                        # f = open(file_fpath,encoding='utf-8-sig')
                        # data = json.load(f)
                        data = json_cor_and_load(file_fpath)
                        inputs_dic['subfiles'][fld_file]['data'] = data
                        load_data(inputs_dic,data,rootdir=rootdir2)
                    elif file_format=='xml':
                        data = etree.parse(file_fpath)
                        inputs_dic['subfiles'][fld_file]['data'] = data
                    else:

                        try:
                            inputs_dic['subfiles'][fld_file]['data'] = pd.read_csv(file_fpath,encoding='utf-8-sig')
                        except:
                            inputs_dic['subfiles'][fld_file]['data'] = pd.read_csv(file_fpath,encoding='unicode_escape')

def load_vecto(vectofile):
    inputs = {}
    inputs['root']= os.path.dirname(vectofile)

    inputs['vecto']={}
    inputs['vecto']['file']=os.path.basename(vectofile)

    try:

        # some json are wrong
        data = json_cor_and_load(vectofile)
        # f = open(vectofile,encoding='utf-8-sig')
        # data = json.load(f)
    except ValueError:
        print(vectofile, ' is not a valid json file')
        return None
    inputs['vecto']['type']='json'

    inputs['vecto']['data']=data

    try:
        body = data['Body']
    except ValueError:
        print(vectofile, ' is not a valid json file')
        return None

    load_data(inputs,body)

    return inputs

def get_inputs_path(inputs_dic):

    lpaths=[]

    for fld,val in inputs_dic.items():

        l_fld=[fld]

        if (isinstance(val,str) or isinstance(val,float)  or isinstance(val,int)) or isinstance(val,pd.DataFrame):
            lpaths = lpaths + l_fld
        elif isinstance(val,dict):
            # print('='*100)
            # print(fld)
            test = get_inputs_path(val)
            lpath_i = []

            for p in test:
                elt2add=[]
                if (isinstance(p,str) or isinstance(p,float)  or isinstance(p,int)):
                    elt2add = l_fld+[p]
                elif p is not None:
                    elt2add = l_fld+p
                lpath_i.append(elt2add)

            # print(l_fld)

            lpaths = lpaths + lpath_i

        elif isinstance(val,list):
            for idx,elt in enumerate(val):
                if isinstance(elt,dict):
                    test = get_inputs_path(elt)
                    lpath_i = []
                    for p in test:
                        elt2add=[]
                        if (isinstance(p,str) or isinstance(p,float)  or isinstance(p,int)):
                            elt2add = l_fld+[idx]+[p]
                        elif p is not None:
                            elt2add = l_fld+[idx]+p
                        lpath_i.append(elt2add)

                    lpaths = lpaths + lpath_i


    return lpaths

def get_dict_val_from_keypath(dico,p):

    if isinstance(p,str):
        p=[p]

    dic_var=dico
    for key in p:
        dic_var = dic_var[key]

    return dic_var



def update_nested_dict(nested_dict, keys, new_value):

   if len(keys) == 1:
      nested_dict[keys[0]] = new_value
   else:
      key = keys[0]
      if (key in nested_dict) or (isinstance(key,int) and isinstance(nested_dict,list)) :
         update_nested_dict(nested_dict[key], keys[1:], new_value)

# def change_file_rootdir(inputs_dic,filename,rootdir):

#     lpath = get_inputs_path(inputs_dic)

#     for p in lpath:

#         val = get_dict_val_from_keypath(inputs_dic,p)

#         if not isinstance(val,str):
#             continue

#         if (filename in val):

#             if rootdir=='.' or rootdir=='':
#                 new_filename = os.path.basename(val)
#             else:
#                 new_filename = os.path.join(rootdir,os.path.basename(val))

#             update_nested_dict(inputs_dic, p, new_filename)

#     return inputs_dic

# def root_all_files(inputs_dic):
#     for p in lpath:

#         val = get_dict_val_from_keypath(inputs_dic,p)

#         if not isinstance(val,str):
#             continue

#         new_filename = os.path.basename(val)

def replace_string_in_filepath(inputs_dic,oldstr,newstr):

    lpath = get_inputs_path(inputs_dic)

    lpath_filename = [p for p in lpath if 'file' in p]
    l_filename = [get_dict_val_from_keypath(inputs_dic,p) for p in lpath_filename]

    for fname in l_filename:
        if not oldstr in fname:
            continue

        l_keys = find_keys_for_value(inputs_dic,fname)
        for key in l_keys:
            current_fpath = get_dict_val_from_keypath(inputs_dic,key)
            new_fpath = current_fpath.replace(oldstr,newstr)
            update_nested_dict(inputs_dic, key, new_fpath)


def find_keys_for_value(inputs_dic,value):

    lpath = get_inputs_path(inputs_dic)
    out = []

    for p in lpath:
        # print(p)
        val = get_dict_val_from_keypath(inputs_dic,p)
        # print(val)

        # if not (isinstance(val,str) or isinstance(val,float) or isinstance(val,int)):
        if not type(val)==type(value):
            continue

        if (value == val) or (isinstance(val,str) and ((value in val) or os.path.normpath(value)==os.path.normpath(val))):
            out.append(p)

    return out

def create_dir_if_not_exist(dir_path):

        pathi = ''
        for pathstep in dir_path.split(os.sep):
            pathi = os.path.join(pathi,pathstep)
            if not os.path.exists(pathi):
                os.mkdir(pathi)

def write_vecto(inputs_dic,outpath):

    lpaths = get_inputs_path(inputs_dic)

    lpath_file = [p for p in lpaths if 'file' in p]

    for fname_path in lpath_file:
        data_path = fname_path[:-1]+['data']
        type_path = fname_path[:-1]+['type']

        fname = get_dict_val_from_keypath(inputs_dic,fname_path)
        typ = get_dict_val_from_keypath(inputs_dic,type_path)

        data = get_dict_val_from_keypath(inputs_dic,data_path)


        f_path = os.path.join(outpath,fname)
        dir2create = os.path.dirname(f_path)

        create_dir_if_not_exist(dir2create)

        if typ=='json':
            with open(f_path, 'w', encoding='utf-8-sig') as f:
                json.dump(data, f, ensure_ascii=False, indent=4)
        elif typ=='xml':
            xml_tree = data

            etree.indent(xml_tree,space='\t',level=0)

            # if not outpath.endswith('.xml'):
            #     outpath = outpath + '.xml'

            xml_tree.write(f_path, encoding='utf-8', xml_declaration=True, pretty_print=False)

        elif typ=='csv':

            data.to_csv(f_path, index=False, encoding='utf-8-sig')




if __name__ == "__main__":


    # file_vecto_fpath = os.path.join('INPUTS','Group 54','ML3van.vecto')

    # inputs = {}
    # inputs['root']= os.path.dirname(file_vecto_fpath)

    # inputs['vecto']={}
    # inputs['vecto']['file']=os.path.basename(file_vecto_fpath)


    # f = open(file_vecto_fpath)
    # data = json.load(f)

    # inputs['vecto']['data']=data

    # inputs2 = copy.deepcopy(inputs)

    # body = data['Body']

    # # load_data(inputs,body)

    # inputs3 = load_vecto(file_vecto_fpath)

    # lpaths3 = get_inputs_path(inputs3)

    # write_vecto(inputs3,'TEST_WRITE')

    # # test changing names

    # test = {'a':'abc','b':{'c':'cde','d':'cde'}}
    # print(test)
    # # update_nested_dict(test, ['b','c'], 'efg')
    # # print(test)
    # # update_nested_dict(test, ['b','c'], 'cde')
    # # print(test)

    # test2 = change_file_rootdir(test,'cde','vecto')
    # print('change_file_rootdir')
    # print(test2)

    # test changing names with real vecto file

    file_vecto_fpath2 = os.path.join('INPUTS2','Generic Vehicles','Declaration Mode','PEV','GenericIEPC','IEPC_Gbx1Speed','IEPC__Gbx1.vecto')

    inputs_ini = load_vecto(file_vecto_fpath2)
    inputs4 = load_vecto(file_vecto_fpath2)

    replace_string_in_filepath(inputs4,'..','.')
    # change_file_rootdir(inputs4,'..','.')
    write_vecto(inputs4,r'TEST_WRITE_RENAME\TEST')

    ptest = find_keys_for_value(inputs4,'IEPC_Gbx1_drag.viepcd')
    ptest2 = find_keys_for_value(inputs4,'..')

