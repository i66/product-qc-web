# -*- coding:utf-8 -*-
import sys
import subprocess
import os
import shutil
import datetime
import io
from git import Repo

REPLACE_TOKEN = '[BUILD_DATE-VERSION]';

def getWorkingDir():
    return os.getcwd();

def getLastCommitShortSha():
    currentDir = getWorkingDir();
    repo = Repo(currentDir);
    sha = repo.head.object.hexsha;
    shortSha = repo.git.rev_parse(sha, short=6);
    print 'Latest commit: ', shortSha;
    return shortSha;

def overrideLayoutCsHtml(versionString):
    inputDir = os.path.join(getWorkingDir(), 'product-qc-web\Views\Shared');
    inFileFullPath = os.path.join(inputDir, '_Layout.cshtml');

    file = io.open(inFileFullPath, 'r', encoding='UTF-8-sig');
    filedata = file.read();
    filedata = filedata.replace(REPLACE_TOKEN, versionString);
    file.close();

    file = io.open(inFileFullPath, 'w', encoding='UTF-8-sig');
    file.write(filedata);
    file.close();

def main(argv):
    argvCount = len(sys.argv);
    print 'Number of arguments:', len(sys.argv), 'arguments.';
    print 'Argument List:', str(sys.argv);
    if argvCount < 2:
        print 'Argv not enough!';
        print 'Usage: debVersion.py [BuildNumber]';
        return 1;

    buildNumber = sys.argv[1];

    shortSha = getLastCommitShortSha();
    if not shortSha:
        print 'Short SHA is Empty!';
        return 1;

    now = datetime.datetime.now()
    versionSuffix =  buildNumber + '.' + shortSha;
    versionString = now.strftime("%Y-%m-%d v") + versionSuffix;

    versionString = versionString.strip();
    print 'Version String:', versionString;
    overrideLayoutCsHtml(versionString);
    return 0;


if __name__ == "__main__":
   result = main(sys.argv[1:])
   sys.exit(result)
